function resultat = train(nbr_obj) 
	%% Cette fonction utilise la librairie LIBSVM 3.2 sous Matlab
	%  
	
	dataset=importdata('C:/Users/Sakher/Desktop/TC1 Apprentissage/TChebyshev_ordre8_20.txt');
	group=dataset(:,1);
	%matrice contient nos données
	data=dataset(:,2:13)
	[~,~,labels] = unique(group);   %# labels: 1/2/3
	%taille des instances et classes
	numInst = size(data,1);
	numLabels = max(labels);


	%devisé aléatoirement les données
	idx = randperm(numInst);
	%choisir le nombre d'instances test/train
	numTrain = 2000; 
	numTest = numInst - numTrain ; %test dans ce cas prends 2500-2000=500 instances aléatoires ,par exemple
	
	%choisir les données de test et train avec leurs classes
	trainData = data(idx(1:numTrain),:);  
	testData = data(idx(numTrain+1:end),:);
	trainLabel = labels(idx(1:numTrain)); 
	testLabel = labels(idx(numTrain+1:end));

	 
	%# Faire l'apprentissage des K models ( un contre tous)
	model = cell(numLabels,1);
	for k=1:numLabels
		model{k} = svmtrain(double(trainLabel==k), trainData, '-c 1 -g 0.1 -b 1 -t 1 -d 2');
		%c =1 // constante qui représente le cout 
		%g=0.1 la veleur de gamma, sinon 1/nbr features
		%b=1 >> SVR
		%t=1 : noyau polynomial avec d=2 
	end

	%# get probability estimates of test instances using each model
	%calculer les probabilité d'estimation de Yi de chaque instance de test en utilisant les K modèles
	prob = zeros(numTest,numLabels);
	for k=1:numLabels
		[~,~,p] = svmpredict(double(testLabel==k), testData, model{k}, '-b 1');
		prob(:,k) = p(:,model{k}.Label==1);    %# probabilité de la classe k
	end

	%# predire la classe ayant la plus grande proba
	[~,pred] = max(prob,[],2);
	acc = sum(pred == testLabel) ./ numel(testLabel)    %# Calcule de la précision
	C = confusionmat(testLabel, pred)                   %# Matrice de confusion
	
	%sauvegarder les données
	%save('training_data_20')
    
	
	%%calcule du rappel et précision et F-mesure
    recall=0
    prec=0
    for i=1:numLabels
        vali=C(i,i)
        sommeRecall=0
        sommePrec=0
        for j=1:numLabels
            sommeRecall=sommeRecall+C(i,j)
            sommePrec=sommePrec+C(j,i)
        end
        recall=recall+((vali*1.0)/(sommeRecall*1.0));
        prec=prec+((vali*1.0)/(sommePrec*1.0));
    end
    prec=prec/(numLabels*1.0);
    recall=recall/(numLabels*1.0);
	fmesure=(2*prec*recall)/(recall+prec);
	
	fmesure
	prec
    recall
end